namespace Vahajagjort.Db

open System.Collections.Generic
open System
open Vahajagjort.Serializer

module Doing =         
   
    // let init<'a> path =
    //     match Serializer.deserialize path with
    //     | Some a -> storage <- a
    //     | None -> sotra

    // let private serialize path =
    //     Serializer.serialize storage path            

    let getAll<'a> (storage:Dictionary<int, 'a>) =
        storage.Values |> Seq.map(id)

    let create<'a> (storage:Dictionary<int, 'a>) (fn: int -> 'a -> 'a) item =
        let id = storage.Values.Count + 1
        let newItem = fn id item
        storage.Add(id, newItem)        
        newItem

    let updateItemById<'a> (storage:Dictionary<int, 'a>) (fn: int -> 'a -> 'a) id item = 
        if storage.ContainsKey(id) then
            storage.[id] <- fn id item            
            Some item
        else
            None
    
    let updateItem<'a> (storage:Dictionary<int, 'a>) (creator: int -> 'a -> 'a) (getId: 'a -> int) item =
        updateItemById storage creator (getId item) item

    let deleteItem<'a> (storage:Dictionary<int, 'a>) id = 
        storage.Remove(id) |> ignore
        
    let getItem<'a> (storage:Dictionary<int, 'a>) id =
        if storage.ContainsKey(id) then
            Some storage.[id]
        else
            None

    let isExists (storage:Dictionary<int, 'a>) = storage.ContainsKey    