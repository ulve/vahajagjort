namespace Vahajagjort.Db

open System.Collections.Generic
open System
open Vahajagjort.Serializer

module DictionaryStorage =                      
    let getAll (storage:Dictionary<int, 'a>) =
        storage.Values |> Seq.map(id)

    let create (storage:Dictionary<int, 'a>) (fn: int -> 'a -> 'a) serializer item =
        let id = storage.Values.Count + 1
        let newItem = fn id item
        storage.Add(id, newItem)     
        serializer storage   
        newItem

    let updateItemById (storage:Dictionary<int, 'a>) (fn: int -> 'a -> 'a) serializer id item = 
        if storage.ContainsKey(id) then
            storage.[id] <- fn id item       
            serializer storage     
            Some item
        else
            None
    
    let updateItem (storage:Dictionary<int, 'a>) (creator: int -> 'a -> 'a) (getId: 'a -> int) serializer item =
        updateItemById storage creator serializer (getId item) item

    let deleteItem (storage:Dictionary<int, 'a>) serializer id = 
        storage.Remove(id) |> ignore
        serializer storage
        ()
        
    let getItem (storage:Dictionary<int, 'a>) id =
        if storage.ContainsKey(id) then
            Some storage.[id]
        else
            None

    let isExists (storage:Dictionary<int, 'a>) = storage.ContainsKey    