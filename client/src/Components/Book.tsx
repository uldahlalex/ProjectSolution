import {useAtom} from "jotai";
import {AllAuthorsAtom} from "../atoms/atoms.ts";
import {type AuthorDto, type BookDto, type UpdateBookRequestDto} from "../generated-client.ts";
import type {BookProps} from "./Books.tsx";
import {useState} from "react";
import useLibraryCrud from "../useLibraryCrud.ts";
import toast from "react-hot-toast";

export function Book(props: BookProps) {
    const [authors] = useAtom(AllAuthorsAtom);
    const libraryCrud = useLibraryCrud();
    const [updateBookForm, setUpdateBookForm] = useState<UpdateBookRequestDto>({
        authorsIds: authors.filter(a => props.book.authorsIds?.includes(a.id)).map(a => a.id!),
        bookIdForLookupReference: props.book.id!,
        genreId: props.book.genre?.id!,
        newTitle: props.book.title!,
        newPageCount: props.book.pages!
    });

    function getAuthorNamesFromIds(ids: string[]): string[] {
        const filtered = authors.filter(a => ids.includes(a.id!));
        const names = filtered.map(f => f.name!);
        return names;
    }

    function updateBook(author: AuthorDto, book: BookDto) {
        libraryCrud.updateBooks(updateBookForm).then(success => {
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
 
            <div className="menu dropdown-content bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                <div>Authors:</div>
                {
                    authors.map(a => <div key={a.id} className="join">
                        <p className="">{a.name}</p>
                        <input className="checkbox join-item checkbox-lg"
                               type="checkbox"
                               defaultChecked={props.book.authorsIds.includes(a.id)}
                               onChange={() => {
                                   const alreadyAssigned = props.book.authorsIds.includes(a.id!);
                                   if (alreadyAssigned) {
                                       const newAuthorIds = props.book.authorsIds.filter(id => id !== a.id);
                                       setUpdateBookForm({...updateBookForm, authorsIds: newAuthorIds});
                                       return;
                                   }
                                   const newAuthorIds = [...(props.book.authorsIds ?? []), a.id!];
                                   setUpdateBookForm({...updateBookForm, authorsIds: newAuthorIds});
                                        
                               }}/>
                    </div>)
                }
                <input className="input" value={updateBookForm.newTitle}
                       onChange={e => setUpdateBookForm({...updateBookForm, newTitle: e.target.value})}/>
                <input className="input" value={updateBookForm.newPageCount} onChange={e => setUpdateBookForm({
                    ...updateBookForm,
                    newPageCount: Number.parseInt(e.target.value)
                })}/>
                <button className="btn btn-primary" onClick={() => libraryCrud.updateBooks(updateBookForm)}>Submit</button>
           
            </div>
        </details>
    </li>;

}