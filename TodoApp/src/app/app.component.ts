import { Component } from '@angular/core';
import { TodoListComponent } from './components/todo-list/todo-list.component';
import { AddTodoComponent } from './components/add-todo/add-todo.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TodoListComponent, AddTodoComponent], // Added AddTodoComponent
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'TodoApp';
}