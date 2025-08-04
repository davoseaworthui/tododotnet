import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';

import { TodoService } from '../../services/todo.service';
import { CreateTodoDto } from '../../models/todo.models';

@Component({
  selector: 'app-add-todo',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule
  ],
  templateUrl: './add-todo.component.html',
  styleUrl: './add-todo.component.scss'
})
export class AddTodoComponent {
  newTodo: CreateTodoDto = {
    title: '',
    description: '',
    priority: 1,
    dueDate: undefined
  };

  isSubmitting = false;

  constructor(private todoService: TodoService) {}

  onSubmit() {
    if (!this.newTodo.title.trim()) {
      return;
    }

    this.isSubmitting = true;

    this.todoService.createTodo(this.newTodo).subscribe({
      next: (createdTodo) => {
        console.log('Todo created:', createdTodo);
        // Reset form
        this.newTodo = {
          title: '',
          description: '',
          priority: 1,
          dueDate: undefined
        };
        this.isSubmitting = false;
        // Emit event to parent component to refresh list
        window.location.reload(); // Simple refresh for now
      },
      error: (error) => {
        console.error('Error creating todo:', error);
        this.isSubmitting = false;
      }
    });
  }

  getPriorityText(priority: number): string {
    switch (priority) {
      case 1: return 'Low';
      case 2: return 'Medium';
      case 3: return 'High';
      default: return 'Low';
    }
  }
}