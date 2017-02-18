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
        
    type Block = {Id: int; Person:string; Text:string; Date:DateTime}

    [<EntryPoint>]
    let main _ =                
        
        let doneWebPart = restish "done" {
            GetAll = Done.getAll
            Create = Done.create
            Update = Done.updateItem
            Delete = Done.deleteItem
            GetById = Done.getItem
            UpdateById = Done.updateItemById
            IsExists = Done.isExists
        }

        Doing.init ()

        let doingWebPart = restish "doing" {
            GetAll = Doing.getAll
            Create = Doing.create
            Update = Doing.updateItem
            Delete = Doing.deleteItem
            GetById = Doing.getItem
            UpdateById = Doing.updateItemById
            IsExists = Doing.isExists
        } 
        
        startWebServer defaultConfig (choose [doneWebPart; doingWebPart])
        0
