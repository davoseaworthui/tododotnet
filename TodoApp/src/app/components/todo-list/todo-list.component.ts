import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip'; // ADD THIS LINE

import { TodoService } from '../../services/todo.service';
import { Todo } from '../../models/todo.models';

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
    MatTooltipModule  // ADD THIS LINE
  ],
  templateUrl: './todo-list.component.html',
  styleUrl: './todo-list.component.scss'
})

export class TodoListComponent implements OnInit {
  todos: Todo[] = [];
  loading = true;
  error: string | null = null;

  constructor(private todoService: TodoService) {}

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
        console.log('Loaded todos:', todos); // For debugging
      },
      error: (error) => {
        this.error = 'Failed to load todos. Make sure your API is running!';
        this.loading = false;
        console.error('Error loading todos:', error);
      }
    });
  }

  toggleTodo(id: number) {
    this.todoService.toggleTodo(id).subscribe({
      next: (updatedTodo) => {
        // Update the todo in our local array
        const index = this.todos.findIndex(t => t.id === id);
        if (index !== -1) {
          this.todos[index] = updatedTodo;
        }
      },
      error: (error) => {
        console.error('Error toggling todo:', error);
      }
    });
  }

  deleteTodo(id: number) {
    if (confirm('Are you sure you want to delete this todo?')) {
      this.todoService.deleteTodo(id).subscribe({
        next: () => {
          // Remove from local array
          this.todos = this.todos.filter(t => t.id !== id);
        },
        error: (error) => {
          console.error('Error deleting todo:', error);
        }
      });
    }
  }

  getPriorityColor(priority: number): string {
    switch (priority) {
      case 1: return 'primary';  // Low - blue
      case 2: return 'accent';   // Medium - accent color
      case 3: return 'warn';     // High - red
      default: return 'primary';
    }
  }
}