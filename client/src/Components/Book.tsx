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

    return <li className="card bg-base-100 shadow-lg border border-base-300 mb-4 hover:shadow-xl transition-shadow duration-200">
        <div className="card-body p-6">
            <div className="flex justify-between items-start">
                <div className="flex-1">
                    <h3 className="card-title text-lg font-bold text-primary mb-2">{props.book.title}</h3>
                    <div className="flex flex-col gap-1">
                        <div className="badge badge-outline badge-sm">
                            📖 {props.book.pages} pages
                        </div>
                        <div className="text-sm text-base-content/70">
                            ✍️ By {getAuthorNamesFromIds(props.book.authorsIds!).join(', ')}
                        </div>
                    </div>
                </div>

                <details className="dropdown dropdown-left">
                    <summary className="btn btn-square btn-outline btn-sm hover:btn-primary">
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="w-4 h-4 stroke-current">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 100 4m0-4v2m0-6V4"></path>
                        </svg>
                    </summary>
 
                    <div className="dropdown-content menu bg-base-100 rounded-box z-10 w-80 p-4 shadow-xl border border-base-300">
                        <div className="mb-4">
                            <h4 className="font-semibold text-lg mb-3 flex items-center gap-2">
                                <span>📝</span> Edit Book
                            </h4>

                            <div className="form-control mb-4">
                                <label className="label">
                                    <span className="label-text font-medium">Title</span>
                                </label>
                                <input
                                    className="input input-bordered w-full"
                                    value={updateBookForm.newTitle}
                                    placeholder="Enter book title"
                                    onChange={e => setUpdateBookForm({...updateBookForm, newTitle: e.target.value})}
                                />
                            </div>

                            <div className="form-control mb-4">
                                <label className="label">
                                    <span className="label-text font-medium">Pages</span>
                                </label>
                                <input
                                    className="input input-bordered w-full"
                                    type="number"
                                    value={updateBookForm.newPageCount}
                                    placeholder="Number of pages"
                                    onChange={e => setUpdateBookForm({
                                        ...updateBookForm,
                                        newPageCount: Number.parseInt(e.target.value)
                                    })}
                                />
                            </div>

                            <div className="form-control">
                                <label className="label">
                                    <span className="label-text font-medium">Authors</span>
                                </label>
                                <div className="space-y-2 max-h-32 overflow-y-auto">
                                    {authors.map(a =>
                                        <label key={a.id} className="flex items-center gap-3 p-2 rounded-lg hover:bg-base-200 cursor-pointer">
                                            <input
                                                className="checkbox checkbox-primary checkbox-sm"
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
                                                }}
                                            />
                                            <span className="text-sm">{a.name}</span>
                                        </label>
                                    )}
                                </div>
                            </div>

                            <div className="divider"></div>
                            <button
                                className="btn btn-primary w-full gap-2"
                                onClick={() => libraryCrud.updateBooks(updateBookForm)}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" className="w-4 h-4 stroke-current">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7"></path>
                                </svg>
                                Update Book
                            </button>
                        </div>
                    </div>
                </details>
            </div>
        </div>
           
       
    </li>;

}