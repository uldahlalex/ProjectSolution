import {useEffect, useState} from "react";
import {type Author, AuthorOrderingOptions} from "./generated-client.ts";
import {useNavigate, useSearchParams} from "react-router";
import {resolveRefs} from "dotnet-json-refs";
import {apiClient} from "./App.tsx";

export function PaginatedAuthors() {

    const [authors, setAuthors] = useState<Author[]>([])
    const [searchParams] = useSearchParams();
    const pageNumber = parseInt(searchParams.get('pageNumber') || '1');
    const itemsPerPage = parseInt(searchParams.get('itemsPerPage') || '10');
    const ordering = Number.parseInt(searchParams.get('ordering') ?? "0") as AuthorOrderingOptions;

    const navigate = useNavigate();

    useEffect(() => {
        apiClient.getAuthors({
            ordering: ordering,
            take: itemsPerPage,
            skip: (itemsPerPage)*(pageNumber-1),
            filters: [{
                
            }]
        }).then(result => {
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
            const newPageNumber = pageNumber - 1;
            navigate('./?itemsPerPage=' + itemsPerPage + '&pageNumber=' + newPageNumber)
        }}>Prev page
        </button>
        Page: {
        pageNumber
    }
        <button onClick={() => {
            const newPageNumber = pageNumber + 1;
            navigate('./?itemsPerPage=' + itemsPerPage + '&pageNumber=' + newPageNumber+'&ordering='+ordering)
        }}>Next page
        </button>
        <input onChange={e => {
            const newItemsPerPage = Number.parseInt(e.target.value)
            navigate('./?itemsPerPage=' + newItemsPerPage + '&pageNumber=' + pageNumber+'&ordering='+ordering)
        }} defaultValue={itemsPerPage}/>

        <select defaultValue={ordering} onChange={e => {
            console.log(e.target.value)
            navigate('./?itemsPerPage=' + itemsPerPage + '&pageNumber=' + pageNumber+'&ordering='+e.target.value)
        }}>
            <option value={AuthorOrderingOptions.Name}>Name</option>
            <option value={AuthorOrderingOptions.NumberOfBooksPublished}>Number of books published</option>
            <option value={AuthorOrderingOptions.NameDescending}>Name, descending</option>
        </select>

    </>
}