import {useEffect, useState} from "react";
import {type Author, type AuthorOrderingOptions, type GetAuthorsRequestDto} from "./generated-client.ts";
import {useNavigate, useSearchParams} from "react-router";
import {resolveRefs} from "dotnet-json-refs";
import {apiClient} from "./ApiClient.tsx";

export function PaginatedAuthors() {

    const [authors, setAuthors] = useState<Author[]>([])
    const [searchParams] = useSearchParams();
    const pageNumber = parseInt(searchParams.get('pageNumber') || '1')
    const itemsPerPage = parseInt(searchParams.get('itemsPerPage') || '2');
    const ordering = Number.parseInt(searchParams.get('ordering') ?? "0") as AuthorOrderingOptions;

    const navigate = useNavigate();

    useEffect(() => {
        const request: GetAuthorsRequestDto = {
            ordering: ordering,
            skip: (pageNumber - 1) * itemsPerPage,
            take: itemsPerPage
        }
        apiClient.getAuthors(request).then(result => {
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
            navigate('./?itemsPerPage=' + itemsPerPage + '&pageNumber=' + newPageNumber)
        }}>Next page
        </button>
        <hr />
        Results per page:
        <input onChange={e => {
            navigate(`?itemsPerPage=${e.target.value}&pageNumber=${pageNumber}&ordering=${ordering}`)
        }} defaultValue={itemsPerPage}/>

        <hr />
        Ordering:
        
        <select defaultValue={ordering!} onChange={e => {
            navigate(`?itemsPerPage=${itemsPerPage}&pageNumber=${pageNumber}&ordering=${ordering}`)
        }}>
            <option value={0}>Order by name</option>
            <option value={1}>Order by most productive author</option>
        </select>
    </>
}