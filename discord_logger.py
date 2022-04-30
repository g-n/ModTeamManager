# Logs Discord messages and inserts them into a MariaDB database

import datetime
import mariadb
import os
import discum

token = os.environ['DISCORD_TOKEN']
db_host = os.environ['DB_HOST']
db_user = os.environ['DB_USER']
db_password = os.environ['DB_PASSWORD']
db_database = os.environ['DB_DATABASE']

conn: mariadb.connection = mariadb.connect(
    host=db_host,
    user=db_user,
    password=db_password,
    database=db_database,
)
cursor = conn.cursor()

bot = discum.Client(token=token, log=False)
guild_names = {} # for resolving guild names, not currently implemented

@bot.gateway.command
def discord_logger(resp):
    """
    Callback event for the Discord API
    :param resp: response json object
    :return:
    """
    if resp.event.ready_supplemental:
        user = bot.gateway.session.user
        print("Logged in as {}#{}".format(user['username'], user['discriminator']))
    if resp.event.message or resp.event.message_updated:
        m = resp.parsed.auto()
        out = {}
        out['message_id'] = int(m['id'])
        if 'author' not in m:
            print(m)
            return
        out['author_id'] = int(m['author']['id'])
        out['author_name'] = m['author']['username']
        guild = int(m['guild_id'])
        channel = int(m['channel_id'])
        out['channel_id'] = int(m['channel_id'])
        # if channel not in guild_names:
        #     guild_names[channel] = bot.gateway.session.guild(str(guild)).channel(str(channel))['name']
        # out['channel_name'] = guild_names[channel]
        out['content'] = m['content']
        if resp.event.message_updated:
            out['edited_at'] = datetime.datetime.fromisoformat(m['edited_timestamp']).strftime('%Y-%m-%d %H:%M:%S')
            out['edited'] = 1
        else:
            out['edited_at'] = ''
            out['edited'] = 0

        out['deleted'] = 0
        out['created_at'] = datetime.datetime.fromisoformat(m['timestamp']).strftime('%Y-%m-%d %H:%M:%S')

        cursor.execute(
            "INSERT INTO ChatMessage (Date, Service, Channel, User, Msg) VALUES (?, ?, ?, ?, ?)",
            (out['created_at'], 'Discord', out['channel_id'], out['author_name'],
             out['content'].strip().encode('utf-8', errors='ignore'))
        )
        conn.commit()

bot.gateway.run(auto_reconnect=True)
