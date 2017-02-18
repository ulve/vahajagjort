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
    open System
        
    type BlockingItem = { Id: int; Person:string; Text:string; Date:DateTime }
    type DoingItem    = { Id: int; Person:string; Text:string; Date:DateTime }   
    type DoneItem     = { Id: int; Person:string; Text:string; Date:DateTime }

    let initStorage path = 
        match Serializer.deserialize path with
        | Some a -> a
        | None -> new Dictionary<int, 'a>()  

    let setupWebPart path name create getId =         
        let storage = initStorage path        
        let serializer = Serializer.serialize path

        restish name {
            GetAll = fun () -> DictionaryStorage.getAll storage
            Create = DictionaryStorage.create storage create serializer
            Update = DictionaryStorage.updateItem storage create getId serializer
            Delete = DictionaryStorage.deleteItem storage serializer
            GetById = DictionaryStorage.getItem storage
            UpdateById = DictionaryStorage.updateItemById storage create serializer
            IsExists = DictionaryStorage.isExists storage
        }

    [<EntryPoint>]
    let main _ =                
        let doneWebPart = setupWebPart "./done.json" "done" (fun a b -> {Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}) (fun a -> a.Id)
        let doingWebPart = setupWebPart "./doing.json" "done" (fun a b -> {DoingItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}) (fun a -> a.Id)
        let blockingWebPart = setupWebPart "./blocking.json" "done" (fun a b -> {BlockingItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}) (fun a -> a.Id)
        
        startWebServer defaultConfig (choose [doneWebPart; doingWebPart; blockingWebPart])
        0
