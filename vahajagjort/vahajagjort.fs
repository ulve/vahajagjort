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
    open System.Collections.Generic
    open System
        
    type BlockItem = { Id: int; Person:string; Text:string; Date:DateTime }
    type DoingItem = { Id: int; Person:string; Text:string; Date:DateTime }   
    type DoneItem  = { Id: int; Person:string; Text:string; Date:DateTime }

    [<EntryPoint>]
    let main _ =                
        let doneStorage = new Dictionary<int, DoneItem>()            
        doneStorage.Add(0, { Id = 0; Person= "olov.johansson@kronofogden.se"; Text = "Fixade SARA-1234"; Date = DateTime.Now});
        let doneCreate a b = {DoneItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}
        let doneGetId a = a.Id

        let doneWebPart = restish "done" {
            GetAll = fun () -> Doing.getAll doneStorage
            Create = Doing.create doneStorage doneCreate
            Update = Doing.updateItem doneStorage doneCreate doneGetId
            Delete = Doing.deleteItem doneStorage
            GetById = Doing.getItem doneStorage
            UpdateById = Doing.updateItemById doneStorage doneCreate
            IsExists = Doing.isExists doneStorage
        }

        let doingStorage = new Dictionary<int, DoingItem>()    
        doingStorage.Add(0, { Id = 0; Person= "paula.berglund@kronofogden.se"; Text = "Fixade SARA-4321"; Date = DateTime.Now});
        let doingCreate a (b:DoingItem) = {DoingItem.Id = a; Person = b.Person; Text = b.Text; Date = DateTime.Now}

        let doingGetId (a:DoingItem) = a.Id

        let doingWebPart = restish "doing" {
            GetAll = fun () -> Doing.getAll doingStorage
            Create = Doing.create doingStorage doingCreate
            Update = Doing.updateItem doingStorage doingCreate doingGetId
            Delete = Doing.deleteItem doingStorage
            GetById = Doing.getItem doingStorage
            UpdateById = Doing.updateItemById doingStorage doingCreate
            IsExists = Doing.isExists doingStorage
        } 
        
        startWebServer defaultConfig (choose [doneWebPart; doingWebPart])
        0
