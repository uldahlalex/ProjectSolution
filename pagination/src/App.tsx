import './App.css'
import {LibraryClient} from "./generated-client.ts";
import {createBrowserRouter, RouterProvider} from "react-router";
import {PaginatedAuthors} from "./PaginatedAuthors.tsx";

export const apiClient = new LibraryClient("http://localhost:5284")




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

