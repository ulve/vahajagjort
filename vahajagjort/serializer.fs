namespace Vahajagjort.Serializer

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.IO

module Serializer = 
    let serialize a (path:string) =
        let jsonSerializerSettings = new JsonSerializerSettings()
        jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
                
        use sw = new StreamWriter(path)        
        JsonConvert.SerializeObject(a, jsonSerializerSettings) |> sw.Write
        
    let deserialize<'a> path =             
        try
            let json = File.ReadAllText(path)
            let a = JsonConvert.DeserializeObject(json, typeof<'a>) :?> 'a
            Some a
        with
        | _ -> None
