namespace Vahajagjort.Program

module Program =
    open Suave.Web
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.WebPart
    open Suave.Utils.Choice
    open Vahajagjort.Reporting
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
            GetAll = fun () -> Doing.getAll doneStorage
            Create = Doing.create doneStorage doneCreate doneSerializer
            Update = Doing.updateItem doneStorage doneCreate doneGetId doneSerializer
            Delete = Doing.deleteItem doneStorage doneSerializer
            GetById = Doing.getItem doneStorage
            UpdateById = Doing.updateItemById doneStorage doneCreate doneSerializer
            IsExists = Doing.isExists doneStorage
        }

        let doingPath = "./doing.json"
        let doingStorage = match Serializer.deserialize doingPath with
                           | Some a -> a
                           | None -> new Dictionary<int, DoingItem>() 
        
        let doingCreate a (b:DoingItem) = {DoingItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}
        let doingGetId (a:DoingItem) = a.Id
        let doingSerializer = Serializer.serialize doingPath

        let doingWebPart = restish "doing" {
            GetAll = fun () -> Doing.getAll doingStorage
            Create = Doing.create doingStorage doingCreate doingSerializer
            Update = Doing.updateItem doingStorage doingCreate doingGetId doingSerializer
            Delete = Doing.deleteItem doingStorage doingSerializer
            GetById = Doing.getItem doingStorage
            UpdateById = Doing.updateItemById doingStorage doingCreate doingSerializer
            IsExists = Doing.isExists doingStorage
        } 
        
        startWebServer defaultConfig (choose [doneWebPart; doingWebPart])
        0
