import {AllAuthorsAtom, AllBooksAtom, AllGenresAtom} from "./atoms/atoms.ts";
import {useAtom} from "jotai";
import type {
    BookDto,
    CreateAuthorRequestDto,
    CreateBookRequestDto,
    CreateGenreDto,
    UpdateAuthorRequestDto,
    UpdateBookRequestDto,
    UpdateGenreRequestDto
} from "./generated-client.ts";
import {LibraryClient} from "./generated-client.ts";
import customCatch from "./customCatch.ts";
import toast from "react-hot-toast";

const isProduction = import.meta.env.PROD;

const prod = "https://projectsolutionserver.fly.dev";
const dev = "http://localhost:5284";

const finalUrl = isProduction ? prod : dev;


const libraryApi = new LibraryClient(finalUrl)

export default function useLibraryCrud() {

    const [authors, setAuthors] = useAtom(AllAuthorsAtom)
    const [books, setBooks] = useAtom(AllBooksAtom)
    const [genres, setGenres] = useAtom(AllGenresAtom)

    async function updateAuthors(dto: UpdateAuthorRequestDto) {
        try {


            const result = await libraryApi.updateAuthor(dto);
            const index = authors.findIndex(a => a.id === result.id);
            if (index > -1) {
                const duplicate = [...authors];
                duplicate[index] = result;
                setAuthors(duplicate);
            }
            toast.success("Author updated successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }

    async function updateBooks(dto: UpdateBookRequestDto) {
        try {
            const result = await libraryApi.updateBook(dto)
            const index = books.findIndex(b => b.id === result.id);
            if (index > -1) {
                const duplicate = [...books];
                duplicate[index] = result;
                setBooks(duplicate);
            }
            toast.success("Book updated successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
            
        }

    }

    async function updateGenres(dto: UpdateGenreRequestDto) {
        try {
            const result = await libraryApi.updateGenre(dto);
            const index = genres.findIndex(g => g.id === result.id);
            if (index > -1) {
                const duplicate = [...genres];
                duplicate[index] = result;
                setGenres(duplicate);
            }
            toast.success("Genre updated successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }

    async function deleteAuthor(id: string) {
        try {
            const result = await libraryApi.deleteAuthor(id);
            const filtered = authors.filter(a => a.id !== id);
            setAuthors(filtered);
            toast.success("Author deleted succesfully successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }

    async function deleteBook(id: string) {
        try {
            const result = await libraryApi.deleteBook(id);
            const filtered = books.filter(b => b.id !== id);
            setBooks(filtered);
            toast.success("Book deleted successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }

    async function deleteGenre(id: string) {
        try {
            const result = await libraryApi.deleteGenre(id);
            const filtered = genres.filter(g => g.id !== id);
            setGenres(filtered);
            toast.success("Genre deleted successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }

    async function createAuthor(dto: CreateAuthorRequestDto) {
        try {
            const result = await libraryApi.createAuthor(dto);
            const duplicate = [...authors];
            duplicate.push(result);
            setAuthors(duplicate);
            toast.success("Author created successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }

    async function createBook(dto: CreateBookRequestDto) {
        try {
            const result = await libraryApi.createBook(dto);
            const duplicate = [...books]
            duplicate.push(result);
            setBooks(duplicate);
            toast.success("Book created successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }

    async function createGenre(dto: CreateGenreDto) {
        try {
            const result = await libraryApi.createGenre(dto);
            const duplicate = [...genres];
            duplicate.push(result);
            setGenres(duplicate);
            toast.success("Genre created successfully");
            return result;
        } catch (e: any) {
            customCatch(e);
        }
    }
    
    async function getAuthors() {
        try {
            const result = await libraryApi.getAuthors();
            setAuthors(result);
        }
        catch (e: any) {
            customCatch(e);
        }
    }
    
    async function getBooks() {
        try {
            const result = await libraryApi.getBooks();
            setBooks(result);
        }
        catch (e: any) {
            customCatch(e);
        }
    }
    
    async function getGenres() {
        try {
            const result = await libraryApi.getGenres();
            setGenres(result);
        }
        catch (e: any) {
            customCatch(e);
        }
    }


    return {
        updateAuthors,
        updateBooks,
        updateGenres,
        deleteAuthor,
        deleteBook,
        deleteGenre,
        createAuthor,
        createBook,
        createGenre,
        getAuthors,
        getBooks,
        getGenres
    }

}