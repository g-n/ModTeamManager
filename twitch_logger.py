# Logs Twitch.tv messages using websockets and inserts them into a MariaDB database

import asyncio
import websockets
import datetime
import os
import mariadb
import sys
import re

twitch_user = sys.argv[1]
twitch_pass = sys.argv[2]
channel = sys.argv[3]
uri = "wss://irc-ws.chat.twitch.tv/"

db_host = os.environ['DB_HOST']
db_user = os.environ['DB_USER']
db_password = os.environ['DB_PASSWORD']
db_database = os.environ['DB_DATABASE']

try:
    conn: mariadb.connection = mariadb.connect(
        host=db_host,
        user=db_user,
        password=db_password,
        database=db_database,

    )
except mariadb.Error as e:
    print(f"Error connecting to chatlog db: {e}")
    sys.exit(1)

cursor = conn.cursor()


def insert_row(date, service, channel, user, msg, user_id):
    """

    :param date: date of the message
    :param service: service i.e. Twitch or Discord
    :param channel: the channel the message was sent in
    :param user: the login username of the user
    :param msg: the message the user sent
    :param user_id: the id of the user
    """
    cursor.execute(
        "INSERT INTO ChatMessage (Date, Service, Channel, User, Msg, UserId) VALUES (?, ?, ?, ?, ?, ?)",
        (date, service, channel, user, msg, user_id)
    )

    cursor.execute(
        "INSERT IGNORE INTO UserProfile (UserId, Login) VALUES (?,?)",
        (user_id, user)
    )
    conn.commit()


async def twitch_logger():
    """
    Connects to Twitch.tv IRC server via websocket and inserts messages into the database
    """
    async with websockets.connect(uri) as ws:
        await ws.send('CAP REQ :twitch.tv/tags')
        await ws.send('PASS {}'.format(twitch_pass))
        await ws.send('NICK {}'.format(twitch_user))
        await ws.send('USER {} 8 * :{}'.format(twitch_user, twitch_user))
        await ws.send('JOIN #{}'.format(channel))
        while True:
            raw = await ws.recv()
            raw = raw.strip()
            raw = raw.splitlines()
            for data in raw:
                if data == 'PING :tmi.twitch.tv':
                    await ws.send('PONG')
                elif data.startswith(':'):
                    print(data)
                elif data.startswith('@'):
                    # System messages
                    match = re.search(r'tmi-sent-ts=(?P<timestamp>\d+).*'
                                      r'user-id=(?P<user_id>\d+).*'
                                      r'\:(?P<username>\w+).*PRIVMSG\s'
                                      r'\#(?P<channel_name>\w+)\s'
                                      r'\:(?P<message>.*)',
                                      data)
                    if match is None:
                        print('no match', data)
                    else:
                        fields = match.groupdict()
                        date = datetime.datetime.utcfromtimestamp(int(fields['timestamp']) / 1000).strftime(
                            '%Y-%m-%d %H:%M:%S')
                        msg = fields['message'].strip().encode('utf-8', errors='ignore')
                        # user = fields['username']
                        # message = fields['message']
                        print('user', fields['timestamp'], fields['user_id'], fields['username'],
                              fields['channel_name'], fields['message'])
                        insert_row(date, 'Twitch', fields['channel_name'], fields['username'], msg, fields['user_id'])
                else:
                    # unhandled message types
                    print('unhandled data format', data)


asyncio.get_event_loop().run_until_complete(twitch_logger())
