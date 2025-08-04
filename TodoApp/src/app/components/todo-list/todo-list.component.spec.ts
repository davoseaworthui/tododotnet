import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { TodoService } from '../../services/todo.service';
import { Todo } from '../../models/todo.models';
import { AddTodoComponent } from '../add-todo/add-todo.component';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDialogModule,
  ],
  templateUrl: './todo-list.component.html',
  styleUrl: './todo-list.component.scss',
})
export class TodoListComponent implements OnInit {
  todos: Todo[] = [];
  loading = true;
  error: string | null = null;

  constructor(
    private todoService: TodoService,
    private dialog: MatDialog // Inject MatDialog
  ) {}

  ngOnInit() {
    this.loadTodos();
  }

  loadTodos() {
    this.loading = true;
    this.error = null;

    this.todoService.getTodos().subscribe({
      next: (todos) => {
        this.todos = todos;
        this.loading = false;
        console.log('Loaded todos:', todos);
      },
      error: (error) => {
        this.error = 'Failed to load todos. Make sure your API is running!';
        this.loading = false;
        console.error('Error loading todos:', error);
      },
    });
  }

  // NEW: Open Add Todo Modal
  openAddTodoDialog() {
    const dialogRef = this.dialog.open(AddTodoComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true,
    });

    // Refresh todo list when dialog closes and todo was created
    dialogRef.afterClosed().subscribe((result) => {
      if (result === 'created') {
        this.loadTodos(); // Refresh the list
      }
    });
  }

  toggleTodo(id: number) {
    this.todoService.toggleTodo(id).subscribe({
      next: (updatedTodo) => {
        const index = this.todos.findIndex((t) => t.id === id);
        if (index !== -1) {
          this.todos[index] = updatedTodo;
        }
      },
      error: (error) => {
        console.error('Error toggling todo:', error);
      },
    });
  }

  deleteTodo(id: number) {
    if (confirm('Are you sure you want to delete this todo?')) {
      this.todoService.deleteTodo(id).subscribe({
        next: () => {
          this.todos = this.todos.filter((t) => t.id !== id);
        },
        error: (error) => {
          console.error('Error deleting todo:', error);
        },
      });
    }
  }

  getPriorityColor(priority: number): string {
    switch (priority) {
      case 1:
        return 'primary';
      case 2:
        return 'accent';
      case 3:
        return 'warn';
      default:
        return 'primary';
    }
  }
}
