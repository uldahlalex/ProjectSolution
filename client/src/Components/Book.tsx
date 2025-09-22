import {useAtom} from "jotai";
import {AllAuthorsAtom, AllBooksAtom} from "../atoms/atoms.ts";
import {type AuthorDto, type BookDto} from "../generated-client.ts";
import type {BookProps} from "./Books.tsx";
import {useState} from "react";
import useLibraryCrud from "../useLibraryCrud.ts";
import toast from "react-hot-toast";

export function Book(props: BookProps) {

    const [books, setBooks] = useAtom(AllBooksAtom);
    const [authors] = useAtom(AllAuthorsAtom);
    const [newAuthorsIdsForBook, setNewAuthorsIdsForBook] = useState<string[]>(props.book.authorsIds || []);
    const libraryCrud = useLibraryCrud();
    
    
    function getAuthorNamesFromIds(ids: string[]): string[] {
        const filtered = authors.filter(a => ids.includes(a.id!));
        const names = filtered.map(f => f.name!);
        return names;
    }

    function updateBook(author: AuthorDto, book: BookDto) {
        const alreadyAssigned = newAuthorsIdsForBook.includes(author.id!);
        let updatedAuthorsIds: string[] = [];
        if (alreadyAssigned) {
            updatedAuthorsIds = newAuthorsIdsForBook.filter(aid => aid !== author.id);
        } else {
            updatedAuthorsIds = [...newAuthorsIdsForBook, author.id!];
        }
        setNewAuthorsIdsForBook(updatedAuthorsIds);
        const dto = {
            authorsIds: newAuthorsIdsForBook,
            bookIdForLookupReference: book.id!,
            genreId: book.genre?.id!,
            newTitle: book.title!,
            newPageCount: book.pages!
        };
        libraryCrud.updateBooks(dto).then(success => {
            toast('Book updated successfully');
        })
        
    }

    return <li className="p-5 list-row w-full flex justify-between">


        <div>
            <div>{props.book.title}</div>
            <div className="text-xs uppercase font-semibold opacity-60">Pages: {props.book.pages}</div>
            <div
                className="text-xs uppercase font-semibold opacity-60">By {getAuthorNamesFromIds(props.book.authorsIds!)}</div>

        </div>
        <details className="dropdown dropdown-left">
            <summary className="btn m-1">⚙️</summary>
            <ul className="menu dropdown-content bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                <li>Assign author to book</li>
                {
                    authors.map(a => <div key={a.id} className="join">
                        <p className="join-item btn btn-disabled">{a.name}</p>
                        <input className="checkbox join-item checkbox-lg" 
                               type="checkbox" 
                               checked={props.book.authorsIds.includes(a.id)} 
                               onChange={() => updateBook(a, props.book)} />
                    </div>)
                }
            </ul>
        </details>
    </li>;

}