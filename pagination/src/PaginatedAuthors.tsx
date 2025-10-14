import {useEffect, useState} from "react";
import {type Author} from "./generated-client.ts";
import {useNavigate, useSearchParams} from "react-router";
import {resolveRefs} from "dotnet-json-refs";
import {apiClient} from "./App.tsx";
import {SieveQueryBuilder} from "ts-sieve-query-builder";

export function PaginatedAuthors() {

    const [authors, setAuthors] = useState<Author[]>([])
    const [searchParams] = useSearchParams();

    const navigate = useNavigate();

    useEffect(() => {
        const q = SieveQueryBuilder.create<Author>()
            .filterContains("name", "Bob_5")
            .sortBy("name")
            .buildFiltersString();

        console.log(q)
        
        apiClient.getAuthors(q, "", 1, 10).then(result => {
            setAuthors(resolveRefs(result));
        })
    }, [searchParams])

    return <>
        {
            authors.map(a => {
                return <div key={a.id}>{a.name}
                    {
                       a.books && a.books.length > 0 && a.books.map(b => {
                            return <span key={b.id}>{b.title}</span>
                        })
                    }

                </div>
            })
        }
        <button onClick={() => {
         //   const newPageNumber = pageNumber - 1;
         //   navigate('./?itemsPerPage=' + itemsPerPage + '&pageNumber=' + newPageNumber)
        }}>Prev page
        </button>
        Page: {
        //pageNumber
    }
        <button onClick={() => {
         //   const newPageNumber = pageNumber + 1;
         //   navigate('./?itemsPerPage=' + itemsPerPage + '&pageNumber=' + newPageNumber+'&ordering='+ordering)
        }}>Next page
        </button>
        <input onChange={e => {
         //   const newItemsPerPage = Number.parseInt(e.target.value)
         //   navigate('./?itemsPerPage=' + newItemsPerPage + '&pageNumber=' + pageNumber+'&ordering='+ordering)
        }}
               //defaultValue={itemsPerPage}
            />

        {/*<select defaultValue={ordering} onChange={e => {*/}
        {/*    console.log(e.target.value)*/}
        {/*    navigate('./?itemsPerPage=' + itemsPerPage + '&pageNumber=' + pageNumber+'&ordering='+e.target.value)*/}
        {/*}}>*/}
        {/*      </select>*/}

    </>
}