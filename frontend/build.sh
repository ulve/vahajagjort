#!/bin/sh
cd ./frontend
yarn
elm-make -y
npm run build
