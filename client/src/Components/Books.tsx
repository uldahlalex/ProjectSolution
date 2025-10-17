import {useEffect, useState} from "react";
import {type Book, type CreateBookRequestDto} from "../generated-client.ts";
import useLibraryCrud from "../useLibraryCrud.ts";
import {BookDetails} from "./BookDetails.tsx";
import {useSearchParams} from "react-router";

export interface BookProps {
    book: Book,
    setAllBooks: React.Dispatch<React.SetStateAction<Book[]>>
}

export default function Books() {

    const [allBooks, setAllBooks] = useState<Book[]>([]);
    const [createBookForm, setCreateBookForm] = useState<CreateBookRequestDto>({
        pages: 1,
        title: "my amazing new book"
    });
    const libraryCrud = useLibraryCrud();
    const [searchParams, setSearchParams] = useSearchParams();
    const filters = searchParams.get('filters') ?? "";
    const sorts = searchParams.get('sorts') ?? "";
    const pageSize = Number.parseInt(searchParams.get('pageSize') ?? "2");
    const page = Number.parseInt(searchParams.get('page') ?? "1");



    useEffect(() => {
        libraryCrud.getBooks(setAllBooks,
            {pageSize: pageSize,
        page: page,
        sorts: sorts,
        filters: filters})
    }, [searchParams])

    return <>
        <div className="p-5">
            <h2 className="text-2xl font-bold text-primary mb-4">📚 Books</h2>

            <div className="card bg-base-100 shadow-lg border border-base-300 mb-6">
                <div className="card-body p-6">
                    <h3 className="card-title text-lg mb-4 flex items-center gap-2">
                        <span>➕</span> Create New Book
                    </h3>
                    <div className="flex gap-4 items-end">
                        <div className="form-control flex-1">
                            <label className="label">
                                <span className="label-text font-medium">Book Title</span>
                            </label>
                            <input
                                value={createBookForm.title}
                                placeholder="Enter book title"
                                className="input input-bordered w-full"
                                onChange={e => setCreateBookForm({...createBookForm, title: e.target.value})}
                            />
                        </div>
                        <div className="form-control w-32">
                            <label className="label">
                                <span className="label-text font-medium">Pages</span>
                            </label>
                            <input
                                value={createBookForm.pages}
                                type="number"
                                placeholder="Pages"
                                className="input input-bordered w-full"
                                onChange={e => setCreateBookForm({...createBookForm, pages: Number.parseInt(e.target.value)})}
                            />
                        </div>
                        <button
                            className="btn btn-primary gap-2"
                            onClick={() => {
                                libraryCrud.createBook(createBookForm, setAllBooks);
                                setCreateBookForm({pages: 1, title: "My Amazing New Book"});
                            }}
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="w-4 h-4 stroke-current">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 4v16m8-8H4"></path>
                            </svg>
                            Create Book
                        </button>
                    </div>
                </div>
            </div>
        </div>
        
        {/*Filter books*/}
        <div className="p-5 pt-0">
            <div className="form-control">
                <label className="label">
                    <span className="label-text font-medium">Search Books</span>
                </label>
                <input
                    placeholder="Search by title..."
                    className="input input-bordered w-full"
                    value={searchParams.get('search') ?? ''}
                    onChange={e => {
                        const newParams = new URLSearchParams(searchParams);
                        if (e.target.value) {
                            newParams.set('filters', `title@=*${e.target.value}`);
                            newParams.set('search', e.target.value);
                        } else {
                            newParams.delete('filters');
                            newParams.delete('search');
                        }
                        newParams.set('page', '1'); // Reset to first page on new search
                        setSearchParams(newParams);
                    }}
                />
            </div>
        </div>

        <ul className="list bg-base-100 rounded-box shadow-md mx-5">
            {
                allBooks && allBooks.length > 0 && allBooks.map(b => <BookDetails key={b.id} book={b} setAllBooks={setAllBooks} />)
            }
        </ul>

        {/*Pagination*/}
        <div className="flex justify-center gap-2 p-5">
            <button
                className="btn btn-sm"
                disabled={page <= 1}
                onClick={() => {
                    const newParams = new URLSearchParams(searchParams);
                    newParams.set('page', (page - 1).toString());
                    setSearchParams(newParams);
                }}
            >
                Previous
            </button>
            <span className="flex items-center px-4">Page {page}</span>
            <button
                className="btn btn-sm"
                disabled={allBooks.length < pageSize}
                onClick={() => {
                    const newParams = new URLSearchParams(searchParams);
                    newParams.set('page', (page + 1).toString());
                    setSearchParams(newParams);
                }}
            >
                Next
            </button>
        </div>
    </>
}
