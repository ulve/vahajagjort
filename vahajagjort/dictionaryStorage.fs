namespace Vahajagjort.Db

open System.Collections.Generic
open System
open Vahajagjort.Serializer

module DictionaryStorage =                      
    let getAll (storage:IDictionary<int, 'a>) =
        storage.Values |> Seq.map(id)

    let create (storage:IDictionary<int, 'a>) (fn: int -> 'a -> 'a) serializer item =
        let id = storage.Values.Count + 1
        let newItem = fn id item
        storage.Add(id, newItem)     
        serializer storage   
        newItem

    let updateItemById (storage:IDictionary<int, 'a>) (fn: int -> 'a -> 'a) serializer id item = 
        if storage.ContainsKey(id) then
            storage.[id] <- fn id item       
            serializer storage     
            Some item
        else
            None
    
    let updateItem (storage:IDictionary<int, 'a>) (creator: int -> 'a -> 'a) (getId: 'a -> int) serializer item =
        updateItemById storage creator serializer (getId item) item

    let deleteItem (storage:IDictionary<int, 'a>) serializer (id:int) = 
        storage.Remove(id) |> ignore
        serializer storage
        ()
        
    let getItem (storage:IDictionary<int, 'a>) itemId =
        if storage.ContainsKey(itemId) then
            Some storage.[itemId]
        else
            None

    let isExists (storage:IDictionary<int, 'a>) = storage.ContainsKey    