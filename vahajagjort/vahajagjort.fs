namespace Vahajagjort.Program

module Program =
    open Suave.Web
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.WebPart
    open Suave.Utils.Choice
    open Vahajagjort.Restish
    open Vahajagjort.Db
    open Vahajagjort.Serializer
    open System.Collections.Generic
    open System.Collections.Concurrent
    open System

    type BlockingItem = { Id: int; Person:string; Text:string; Date:DateTime }
    type DoingItem    = { Id: int; Person:string; Text:string; Date:DateTime }
    type DoneItem     = { Id: int; Person:string; Text:string; Date:DateTime }

    let initStorage path =
        match Serializer.deserialize path with
        | Some a -> a
        | None -> new ConcurrentDictionary<int, 'a>()

    let setupWebPart path name create getId =
        let storage = initStorage path
        let agent = MailboxProcessor.Start(fun inbox ->
            let rec messageLoop () = async {
                let! msg = inbox.Receive()
                Serializer.serialize path msg
                return! messageLoop ()
                }
            messageLoop()
        )

        restish name {
            GetAll = fun () -> DictionaryStorage.getAll storage
            Create = DictionaryStorage.create storage create agent.Post
            Update = DictionaryStorage.updateItem storage create getId agent.Post
            Delete = DictionaryStorage.deleteItem storage agent.Post
            GetById = DictionaryStorage.getItem storage
            UpdateById = DictionaryStorage.updateItemById storage create agent.Post
            IsExists = DictionaryStorage.isExists storage
        }

    [<EntryPoint>]
    let main _ =
        let doneWebPart = setupWebPart "./done.json" "done" (fun a b -> {Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}) (fun a -> a.Id)
        let doingWebPart = setupWebPart "./doing.json" "doing" (fun a b -> {DoingItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}) (fun a -> a.Id)
        let blockingWebPart = setupWebPart "./blocking.json" "blocking" (fun a b -> {BlockingItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}) (fun a -> a.Id)

        startWebServer defaultConfig (choose [doneWebPart; doingWebPart; blockingWebPart])
        0