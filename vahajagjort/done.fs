namespace Vahajagjort.Db

open System.Collections.Generic
open System

type DoneItem = {Id: int; Person:string; Text:string; Date:DateTime}

module Done =     
    let private storage = new Dictionary<int, DoneItem>()
    storage.Add(0, { Id = 0; Person= "olov.johansson@kronofogden.se"; Text = "Fixade SARA-1234"; Date = DateTime.Now})

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
            Some newItem
        else
            None
    
    let updateItem item =
        updateItemById item.Id item

    let deleteItem id = 
        storage.Remove(id) |> ignore

    let getItem id =
        if storage.ContainsKey(id) then
            Some storage.[id]
        else
            None

    let isExists = storage.ContainsKey    