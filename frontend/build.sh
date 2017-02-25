#!/bin/sh
cd ./frontend
yarn
elm-make --yes
npm run build
