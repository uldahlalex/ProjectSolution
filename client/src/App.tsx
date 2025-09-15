import {createBrowserRouter, RouterProvider} from "react-router";
import Home from "./Components/Home.tsx";


function App() {

  return (
    <>
      <RouterProvider router={createBrowserRouter([
          {
              path: '',
              element: <Home />
          }
      ])} />
    </>
  )
}

export default App
