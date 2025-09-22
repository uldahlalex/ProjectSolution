import {createBrowserRouter, RouterProvider} from "react-router";
import Home from "./Components/Home.tsx";
import {DevTools} from "jotai-devtools";
import {useEffect} from "react";
import {useAtom} from "jotai";
import 'jotai-devtools/styles.css'
import {AllAuthorsAtom, AllBooksAtom, AllGenresAtom} from "./atoms/atoms.ts";
import Books from "./Components/Books.tsx";
import Authors from "./Components/Authors.tsx";
import Genres from "./Components/Genres.tsx";
import toast, {Toaster} from "react-hot-toast";
import {ApiException} from "./generated-client.ts";
import type {ProblemDetails} from "./problemdetails.ts";
import useLibraryCrud from "./useLibraryCrud.ts";


function App() {

    const libraryCrud = useLibraryCrud();

    useEffect(() => {
        libraryCrud.getAuthors();
        libraryCrud.getBooks();
        libraryCrud.getGenres();
    }, [])
    

return (
    <>
        <RouterProvider router={createBrowserRouter([
            {
                path: '',
                element: <Home/>,
                children: [
                    {
                        path: 'books',
                        element: <Books/>
                    },
                    {
                        path: 'authors',
                        element: <Authors/>
                    },
                    {
                        path: 'genres',
                        element: <Genres/>
                    }
                ]
            }
        ])}/>
        <DevTools/>
        <Toaster
            position="top-center"
            reverseOrder={false}
        />
    </>
)
}

export default App
