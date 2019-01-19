## Telegram bot that deletes stickers\gifs in groups

To run on raspberry pi:

```
docker build . --tag <tag>
docker run -d --name <name> --restart=unless-stopped <tag>
```