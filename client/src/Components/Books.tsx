import {useAtom} from "jotai";
import {AllAuthorsAtom, AllBooksAtom} from "../atoms/atoms.ts";
import {useState} from "react";
import {type BookDto, type CreateBookRequestDto} from "../generated-client.ts";
import {Book} from "./Book.tsx";
import useLibraryCrud from "../useLibraryCrud.ts";

export interface BookProps {
    book: BookDto
}

export default function Books() {

    const [books, setAllBooks] = useAtom(AllBooksAtom);
    const [authors] = useAtom(AllAuthorsAtom);
    const [createBookForm, setCreateBookForm] = useState<CreateBookRequestDto>({
        pages: 1,
        title: "my amazing new book"
    });
    const libraryCrud = useLibraryCrud();
    

    return <>
        <ul className="list bg-base-100 rounded-box shadow-md">

            <li className="p-4 pb-2 text-xs opacity-60 tracking-wide">All books</li>
                {
                    books.map(b => <Book key={b.id} book={b} />)
                }
        </ul>
       
        <input value={createBookForm.title} placeholder="title" className="input"
               onChange={e => setCreateBookForm({...createBookForm, title: e.target.value})}/>
        <input value={createBookForm.pages} type="number" placeholder="page count" className="input"
               onChange={e => setCreateBookForm({...createBookForm, pages: Number.parseInt(e.target.value)})}/>
        <button className="btn btn-primary" onClick={() => {
            libraryCrud.createBook(createBookForm);
        }}>Create book
        </button>
    </>
}
