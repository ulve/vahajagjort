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
        
    type BlockItem = { Id: int; Person:string; Text:string; Date:DateTime }
    type DoingItem = { Id: int; Person:string; Text:string; Date:DateTime }   
    type DoneItem  = { Id: int; Person:string; Text:string; Date:DateTime }

    [<EntryPoint>]
    let main _ =                
        let donePath = "./done.json"
        let doneStorage = match Serializer.deserialize donePath with
                          | Some a -> a
                          | None -> new Dictionary<int, DoneItem>() 
        
        let doneCreate a b = {DoneItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}
        let doneGetId a = a.Id        
        let doneSerializer = Serializer.serialize donePath

        let doneWebPart = restish "done" {
            GetAll = fun () -> DictionaryStorage.getAll doneStorage
            Create = DictionaryStorage.create doneStorage doneCreate doneSerializer
            Update = DictionaryStorage.updateItem doneStorage doneCreate doneGetId doneSerializer
            Delete = DictionaryStorage.deleteItem doneStorage doneSerializer
            GetById = DictionaryStorage.getItem doneStorage
            UpdateById = DictionaryStorage.updateItemById doneStorage doneCreate doneSerializer
            IsExists = DictionaryStorage.isExists doneStorage
        }

        let doingPath = "./doing.json"
        let doingStorage = match Serializer.deserialize doingPath with
                           | Some a -> a
                           | None -> new Dictionary<int, DoingItem>() 
        
        let doingCreate a (b:DoingItem) = {DoingItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}
        let doingGetId (a:DoingItem) = a.Id
        let doingSerializer = Serializer.serialize doingPath

        let doingWebPart = restish "doing" {
            GetAll = fun () -> DictionaryStorage.getAll doingStorage
            Create = DictionaryStorage.create doingStorage doingCreate doingSerializer
            Update = DictionaryStorage.updateItem doingStorage doingCreate doingGetId doingSerializer
            Delete = DictionaryStorage.deleteItem doingStorage doingSerializer
            GetById = DictionaryStorage.getItem doingStorage
            UpdateById = DictionaryStorage.updateItemById doingStorage doingCreate doingSerializer
            IsExists = DictionaryStorage.isExists doingStorage
        } 
        
        startWebServer defaultConfig (choose [doneWebPart; doingWebPart])
        0
