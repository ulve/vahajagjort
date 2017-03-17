module Main exposing (..)

import Html exposing (..)
import Html.Events exposing (..)
import Http
import Html exposing (beginnerProgram)
import Json.Decode exposing (Decoder, list, map3, string, int, field)


type alias Model =
    { done : Done
    }


type Done
    = Fetching
    | Error
    | Done (List DoneMsg)


type Msg
    = FetchDoneTasks
    | DoneTasks (Result Http.Error (List DoneMsg))


type alias DoneMsg =
    { id : Int
    , person : String
    , text : String
    }



-- Init


init : ( Model, Cmd Msg )
init =
    ( Model Fetching, getDone )



-- Update


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        FetchDoneTasks ->
            ( { model | done = Fetching }, getDone )

        DoneTasks (Ok d) ->
            ( { model | done = Done d }, Cmd.none )

        DoneTasks (Err _) ->
            ( { model | done = Error }, Cmd.none )


getDone : Cmd Msg
getDone =
    let
        request =
            Http.get "http://127.0.0.1:8080/done" decodeDone
    in
        Http.send DoneTasks request


decodeDone : Decoder (List DoneMsg)
decodeDone =
    list
        (map3 DoneMsg
            (field "id" int)
            (field "person" string)
            (field "text" string)
        )



-- View


view : Model -> Html Msg
view model =
    div []
        [ h2 [] [ text "testar" ]
        , button [ onClick FetchDoneTasks ] [ text "More Please!" ]
        , div []
            [ case model.done of
                Fetching ->
                    text "fetching"

                Done d ->
                    Html.ul [] (List.map viewDone d)
                                                

                Error ->
                    text "error"
            ]
        ]

viewDone doneItem =
    Html.li [] [text (doneItem.person ++ ": " ++ doneItem.text)]



subscriptions : Model -> Sub Msg
subscriptions model =
    Sub.none


main =
    Html.program
        { init = init
        , view = view
        , update = update
        , subscriptions = subscriptions
        }
