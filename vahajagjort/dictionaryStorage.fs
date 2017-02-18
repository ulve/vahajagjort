namespace Vahajagjort.Db

open System.Collections.Generic
open System
open Vahajagjort.Serializer

module DictionaryStorage =                      
    let getAll<'a> (storage:Dictionary<int, 'a>) =
        storage.Values |> Seq.map(id)

    let create<'a> (storage:Dictionary<int, 'a>) (fn: int -> 'a -> 'a) serializer item =
        let id = storage.Values.Count + 1
        let newItem = fn id item
        storage.Add(id, newItem)     
        serializer storage   
        newItem

    let updateItemById<'a> (storage:Dictionary<int, 'a>) (fn: int -> 'a -> 'a) serializer id item = 
        if storage.ContainsKey(id) then
            storage.[id] <- fn id item       
            serializer storage     
            Some item
        else
            None
    
    let updateItem<'a> (storage:Dictionary<int, 'a>) (creator: int -> 'a -> 'a) (getId: 'a -> int) serializer item =
        updateItemById storage creator serializer (getId item) item

    let deleteItem<'a> (storage:Dictionary<int, 'a>) serializer id = 
        storage.Remove(id) |> ignore
        serializer storage
        ()
        
    let getItem<'a> (storage:Dictionary<int, 'a>) id =
        if storage.ContainsKey(id) then
            Some storage.[id]
        else
            None

    let isExists (storage:Dictionary<int, 'a>) = storage.ContainsKey    