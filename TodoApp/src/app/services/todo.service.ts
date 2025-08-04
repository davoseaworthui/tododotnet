import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo, CreateTodoDto, UpdateTodoDto, TodoFilterDto, TodoStats } from '../models/todo.models';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = 'http://localhost:5255/api/todo'; // Your .NET API URL

  constructor(private http: HttpClient) { }

  // Get all todos with optional filtering
  getTodos(filter?: TodoFilterDto): Observable<Todo[]> {
    let params = new HttpParams();
    
    if (filter) {
      if (filter.isCompleted !== undefined) {
        params = params.set('isCompleted', filter.isCompleted.toString());
      }
      if (filter.priority) {
        params = params.set('priority', filter.priority.toString());
      }
      if (filter.searchTerm) {
        params = params.set('searchTerm', filter.searchTerm);
      }
      if (filter.page) {
        params = params.set('page', filter.page.toString());
      }
      if (filter.pageSize) {
        params = params.set('pageSize', filter.pageSize.toString());
      }
    }

    return this.http.get<Todo[]>(this.apiUrl, { params });
  }

  // Get a specific todo by ID
  getTodo(id: number): Observable<Todo> {
    return this.http.get<Todo>(`${this.apiUrl}/${id}`);
  }

  // Create a new todo
  createTodo(todo: CreateTodoDto): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, todo);
  }

  // Update an existing todo
  updateTodo(id: number, todo: UpdateTodoDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, todo);
  }

  // Toggle todo completion status
  toggleTodo(id: number): Observable<Todo> {
    return this.http.patch<Todo>(`${this.apiUrl}/${id}/toggle`, {});
  }

  // Delete a todo
  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Get todo statistics
  getStats(): Observable<TodoStats> {
    return this.http.get<TodoStats>(`${this.apiUrl}/stats`);
  }
}