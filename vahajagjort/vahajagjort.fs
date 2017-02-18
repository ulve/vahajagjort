namespace Vahajagjort.Program

module Program =
    open Suave.Web
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.WebPart
    open Suave.Utils.Choice
    open Vahajagjort.Reporting

    [<EntryPoint>]
    let main _ =
        let tasksWebPart = restish {
            GetAll = fun () -> Seq.empty
            Create = fun a -> a
            Update = fun a -> Some a
            Delete = fun a -> ()       
            GetById = fun a -> Some a
            UpdateById = fun a b -> Some b
            IsExists = fun a -> true
        }

        startWebServer defaultConfig tasksWebPart
        0
