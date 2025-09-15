import {createBrowserRouter, RouterProvider} from "react-router";
import Home from "./Components/Home.tsx";
import {DevTools} from "jotai-devtools";
import 'jotai-devtools/styles.css';


function App() {

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
