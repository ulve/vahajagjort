namespace Vahajagjort.Reporting

[<AutoOpen>]
module Restish =
    open Microsoft.FSharp.Core
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization
    open Suave
    open Suave.Filters
    open Suave.Operators
    open Suave.Successful
    open Suave.WebPart
    open Suave.RequestErrors    
    open System.Runtime
    
    type RestResouce<'a> = {
        GetAll : unit -> 'a seq
        Create : 'a -> 'a
        Update : 'a -> 'a option
        Delete : int -> unit
        GetById : int -> 'a option
        UpdateById : int -> 'a -> 'a option
        IsExists : int -> bool
    }

    let toJson v =
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
        
        JsonConvert.SerializeObject(v, jsonSerializerSettings) |> OK
        >=> Writers.setMimeType "application/json; charset=utf-8"

    let fromJson<'a> json = 
        JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a

    let getResourceFromReq<'a> (req : HttpRequest) =
        let getString rawForm =
            System.Text.Encoding.UTF8.GetString(rawForm)
        req.rawForm |> getString |> fromJson<'a>

    let restish resource = 
        let resourcePath = "/reporting"

        let badRequest = BAD_REQUEST "Resource not found"
        
        let resourceIdPath =
            new PrintfFormat<(int -> string),unit,string,string,int>(resourcePath + "/%d")

        let getAll = warbler (fun _ -> resource.GetAll() |> toJson)
        
        let handleResource requestError = function
            | Some r -> r |> toJson
            | _ -> requestError
        
        let deleteResouceById id =
            resource.Delete id
            NO_CONTENT

        let getResourceById = 
            resource.GetById >> handleResource (NOT_FOUND "Resource not found")

        let updateResourceById id = 
            request (getResourceFromReq >> (resource.UpdateById id) >> handleResource badRequest)

        let isResourceExists id =
            if resource.IsExists id then OK "" else NOT_FOUND ""

        choose [
            path resourcePath >=>  
                choose [
                    GET >=> getAll
                    POST >=> 
                        request (getResourceFromReq >> resource.Create >> toJson)
                    PUT >=>
                        request (getResourceFromReq >> resource.Update >> handleResource badRequest)
            ]
            DELETE >=> pathScan resourceIdPath deleteResouceById
            GET >=> pathScan resourceIdPath getResourceById
            PUT >=> pathScan resourceIdPath updateResourceById
            HEAD >=> pathScan resourceIdPath isResourceExists
        ]