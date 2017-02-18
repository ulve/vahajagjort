namespace Vahajagjort.Db

open System.Collections.Generic
open System
open Vahajagjort.Serializer

type DoingItem = {Id: int; Person:string; Text:string; Date:DateTime}

module Doing =         
    let mutable private storage = new Dictionary<int, DoingItem>()

    let private path = "./doing.json"
    
    let init () =
        match Serializer.deserialize path with
        | Some a -> storage <- a
        | None -> ()

    let private serialize path =
        Serializer.serialize storage path            

    let getAll () =
        storage.Values |> Seq.map(id)

    let create item =
        let id = storage.Values.Count + 1
        let newItem = {
            Id = id
            Person = item.Person
            Text = item.Text
            Date = DateTime.Now
        }
        storage.Add(id, newItem)
        serialize path
        newItem

    let updateItemById id item = 
        if storage.ContainsKey(id) then
            let newItem = {
                Id = id
                Person = item.Person
                Text = item.Text
                Date = DateTime.Now
            }
            storage.[id] <- newItem
            serialize path
            Some newItem
        else
            None
    
    let updateItem item =
        updateItemById item.Id item
    let deleteItem id = 
        storage.Remove(id) |> ignore
        serialize path
    let getItem id =
        if storage.ContainsKey(id) then
            Some storage.[id]
        else
            None

    let isExists = storage.ContainsKey    