import './App.css'
import {createBrowserRouter, RouterProvider} from "react-router";
import {PaginatedAuthors} from "./PaginatedAuthors.tsx";


export default function App() {
    
    

  return (
    <>
        
        <RouterProvider router={createBrowserRouter([
            {
                path: '/',
                element: <PaginatedAuthors />
            }
        ]) } />
     </>
  )
}

