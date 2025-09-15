import {createBrowserRouter, RouterProvider} from "react-router";
import Home from "./Components/Home.tsx";
import {DevTools} from "jotai-devtools";
import {useEffect} from "react";
import {libraryApi} from "./api-clients.ts";
import {useAtom} from "jotai";
import 'jotai-devtools/styles.css'
import {AllAuthorsAtom, AllBooksAtom, AllGenresAtom} from "./atoms/atoms.ts";
import Books from "./Components/Books.tsx";
import Authors from "./Components/Authors.tsx";
import Genres from "./Components/Genres.tsx";
import {Toaster} from "react-hot-toast";



function App() {

    const [, setAuthors] = useAtom(AllAuthorsAtom)
    const [, setBooks] = useAtom(AllBooksAtom)
    const [, setGenres] = useAtom(AllGenresAtom)

    useEffect(() => {
        initializeData();
    }, [])

    async function initializeData() {
        setAuthors(await libraryApi.getAuthors());
        setBooks(await libraryApi.getBooks())
        setGenres(await libraryApi.getGenres())
    }

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
