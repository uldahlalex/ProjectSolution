import {createBrowserRouter, RouterProvider} from "react-router";
import Home from "./Components/Home.tsx";
import {DevTools} from "jotai-devtools";
import 'jotai-devtools'
import {useEffect} from "react";
import {libraryApi} from "./api-clients.ts";


function App() {
    
    useEffect(() => {
         libraryApi.getAuthors().then(res => {
            console.log(res)
        }).catch(e => {
            console.log(e)
        })
    }, [])

  return (
    <>
      <RouterProvider router={createBrowserRouter([
          {
              path: '',
              element: <Home />
          }
      ])} />
        <DevTools />
    </>
  )
}

export default App
