## Telegram bot that deletes stickers\gifs in groups

To run on raspberry pi:
1) Add `bot_config.json` config file in `VoidBot\`
```
{
  "Token": "...",
  "Socks5Proxy": {
    "Hostname": "...",
    "Port": "...",
    "Username": "...",
    "Password": "..."
  }
}

```
2) Run
```
docker build . --tag <tag>
docker run -d --name <name> --restart=unless-stopped <tag>
```