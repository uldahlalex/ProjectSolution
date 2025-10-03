import {useEffect, useState} from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import {type Author, LibraryClient} from "./generated-client.ts";
import {createBrowserRouter, RouterProvider, useNavigate, useParams, useSearchParams} from "react-router";
import {resolveRefs} from "dotnet-json-refs";

const apiClient = new LibraryClient("http://localhost:5284")




function PaginatedAuthors() {
    
    const [authors, setAuthors] = useState<Author[]>([])
    const [searchParams] = useSearchParams();
    const pageNumber = parseInt(searchParams.get('pageNumber') || '1');
    const itemsPerPage = parseInt(searchParams.get('itemsPerPage') || '10');
    const navigate = useNavigate();
    
    useEffect(() => {
        apiClient.getAuthors(
            //skip
            (pageNumber-1)*itemsPerPage
            , itemsPerPage).then(result => {
            setAuthors(resolveRefs(result));
        })
    }, [searchParams])
    
    return <>
        {
            authors.map(a => {
                return <div key={a.id}>{a.name}
                    {
                        a.books.map(b => {
                            return <span key={b.id}>{b.title}</span>
                        })
                    }
                
                </div>
            })
        }
        <button onClick={() => {
            const newPageNumber = pageNumber-1;
            navigate('./?itemsPerPage='+itemsPerPage+'&pageNumber='+newPageNumber)
        }}>Prev page</button>
        Page: {
            pageNumber
        }
        <button onClick={() => {
            const newPageNumber = pageNumber+1;
            navigate('./?itemsPerPage='+itemsPerPage+'&pageNumber='+newPageNumber)
        }}>Next page</button>
        <input onChange={e => {
            const newItemsPerPage = Number.parseInt(e.target.value)
            navigate('./?itemsPerPage='+newItemsPerPage+'&pageNumber='+pageNumber)
        }} defaultValue={itemsPerPage} />
        
    </>
}

function App() {
    
    

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

export default App
