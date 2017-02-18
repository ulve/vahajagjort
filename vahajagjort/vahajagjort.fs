namespace Vahajagjort.Program

module Program =
    open Suave.Web
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.WebPart
    open Suave.Utils.Choice
    open Vahajagjort.Reporting
    open System

    [<EntryPoint>]
    let main _ =
        let doneWriter a =
            printfn "Hejdå"
            Some a

        let doneWebPart = restish "done" {
            GetAll = fun () -> Seq.empty
            Create = fun a -> a
            Update = fun a -> Some a
            Delete = fun a -> ()       
            GetById = doneWriter
            UpdateById = fun a b -> Some b
            IsExists = fun a -> true
        }

        let doingWriter a =
            printfn "Hej"
            Some a

        let doingWebPart = restish "doing" {
            GetAll = fun () -> Seq.empty
            Create = fun a -> a
            Update = fun a -> Some a
            Delete = fun a -> ()       
            GetById = doingWriter
            UpdateById = fun a b -> Some b
            IsExists = fun a -> true
        } 
        
        startWebServer defaultConfig (choose [doneWebPart;doingWebPart])
        0
