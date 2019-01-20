open Funogram.Api
open Funogram.Bot
open System.Net.Http
open MihaZupan
open Newtonsoft.Json
open System.IO
open System.Threading.Tasks
open System.Threading

[<AutoOpen>]
module Operators =
    let (^) f x = f x

[<CLIMutable>]
type Socks5Configuration = {
    Hostname: string
    Port: int
    Username: string
    Password: string
}

[<CLIMutable>]
type BotConfig = {
    Socks5Proxy: Socks5Configuration
    Token: string
}

let gifMimeType = "video/mp4"

type MessageAction = 
    | Delete of int64
    | Skip

let onUpdate (context: UpdateContext) =
    context.Update.Message
    |> Option.iter ^fun message ->
        let action =
            let checkIfGif() = 
                message.Document
                |> Option.bind ^ fun document -> document.MimeType
                |> Option.map ^ fun mimeType -> 
                    if mimeType = gifMimeType then Delete message.MessageId
                    else Skip
                |> Option.defaultValue Skip
            let sticker = 
                message.Sticker
                |> Option.map ^ fun _ -> Delete message.MessageId
            Option.defaultWith checkIfGif sticker
        match action with
        | Skip -> ()
        | Delete messageId ->
            deleteMessage message.Chat.Id messageId
            |> api context.Config
            |> Async.RunSynchronously
            |> ignore

let createHttpClient config =
    let messageHandler = new HttpClientHandler()
    messageHandler.Proxy <- HttpToSocks5Proxy(config.Hostname, config.Port, config.Username, config.Password)
    messageHandler.UseProxy <- true
    new HttpClient(messageHandler)

[<EntryPoint>]
let main argv =
    let configFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "bot_config.json"))
    let config = JsonConvert.DeserializeObject<BotConfig>(configFile)

    let botConfiguration = { 
        defaultConfig with 
            Token = config.Token
            Client = createHttpClient config.Socks5Proxy
            AllowedUpdates = ["message"] |> Seq.ofList |> Some
    }

    async {
        printfn "Starting bot"
        do! startBot botConfiguration onUpdate None |> Async.StartChild |> Async.Ignore
        printfn "Bot started"
        do! Task.Delay(Timeout.InfiniteTimeSpan) |> Async.AwaitTask
    } |> Async.RunSynchronously
    printfn "Bot exited"
    0 // return an integer exit code
