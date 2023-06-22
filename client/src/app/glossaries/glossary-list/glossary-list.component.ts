import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { GlossaryTerm } from 'src/app/_models/glossaryTerm';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { TemplateRef } from '@angular/core';
import { faPenSquare } from '@fortawesome/free-solid-svg-icons';
import { faTrash } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-glossary-list',
  templateUrl: './glossary-list.component.html',
  styleUrls: ['./glossary-list.component.css'],
})
export class GlossaryListComponent implements OnInit {
  glossaries: any[] = [];
  newEntry: any = {};
  searchByIdTerm: any;
  updatedGlossary: any = {};
  glossaryTerm: GlossaryTerm | undefined;
  modalRef: BsModalRef | undefined;
  editModal: BsModalRef | undefined;
  modal: any;
  faPenSquare = faPenSquare;
  faTrash = faTrash;
  selectedMonth: string = '';
  pastMonths: string[] = [];

  constructor(private http: HttpClient, private modalService: BsModalService) {}

  ngOnInit(): void {
    // Call the initial methods on component initialization
    this.getGlossaries();

    // Generate past months for dropdown
    const currentDate = new Date();
    for (let i = 1; i <= 12; i++) {
      const pastDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth() - i,
        1
      );
      const monthName = pastDate.toLocaleString('en-US', { month: 'long' });
      this.pastMonths.push(monthName);
    }
  }

  // Fetch glossaries from the API
  getGlossaries() {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http
      .get<any[]>('https://localhost:5001/api/glossary', { headers })
      .subscribe({
        next: (response) => {
          this.glossaries = response;
        },
        error: (error) => {
          console.log(error);
        },
        complete: () => {
          console.log('request has completed');
        },
      });
  }

  // Search glossaries by selected month
  searchByMonth() {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const monthIndex = this.pastMonths.findIndex(
      (month) => month === this.selectedMonth
    );

    if (monthIndex !== -1) {
      const currentDate = new Date();
      const searchDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth() - monthIndex,
        1
      );

      // Perform the search using the searchDate
      const params = new HttpParams().set('searchDate', searchDate.toISOString());

      this.http
        .get<any[]>('http:localhost:5001/api/glossary', {
          headers,
          params,
        })
        .subscribe({
          next: (response) => (this.glossaries = response),
          error: (error) => console.log(error),
        });
    } else {
      // If "All Months" is selected, retrieve all glossaries
      this.getGlossaries();
    }
  }

  // Search glossaries by ID
  searchById() {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const searchTerm = this.searchByIdTerm;

    if (searchTerm) {
      this.http
        .get<any[]>(`http:localhost:5001/api/glossary/${searchTerm}`, {
          headers,
        })
        .subscribe({
          next: (response) => {
            // Filter the glossaries array based on the searched ID
            this.glossaries = response.filter(
              (glossary) => glossary.id === searchTerm
            );
          },
          error: (error) => console.log(error),
        });
    } else {
      // If no search term is provided, retrieve all glossaries
      this.getGlossaries();
    }
  }

  // Open add entry dialog
  openAddDialog(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  // Add a new glossary entry
  addEntry() {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const newEntry = {
      id: this.newEntry.id,
      date: this.newEntry.date,
      term: this.newEntry.term,
      definition: this.newEntry.definition,
      username: this.newEntry.username,
    };

    this.http
      .post<any>('https://localhost:5001/glossary', newEntry, { headers })
      .subscribe({
        next: (response) => {
          this.glossaries.push(response);
          this.modalRef?.hide();
          // Reset the newEntry object to clear the form fields
          this.newEntry = {
            id: null,
            date: null,
            term: '',
            definition: '',
            username: '',
          };
        },
        error: (error) => {
          console.log(error);
        },
      });
  }

  // Open edit glossary modal
  openEditModal(template: TemplateRef<any>, glossary: any) {
    this.updatedGlossary = {
      id: glossary.id,
      date: glossary.date,
      term: glossary.term,
      definition: glossary.definition,
      username: glossary.username,
    };

    this.editModal = this.modalService.show(template);
  }

  // Edit a glossary entry
  editGlossary(modal: any) {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    const updatedGlossary = {
      id: this.updatedGlossary.id,
      date: this.updatedGlossary.date,
      term: this.updatedGlossary.term,
      definition: this.updatedGlossary.definition,
      username: this.updatedGlossary.username,
    };

    this.http
      .put<any>(
        'https://localhost:5001/api/glossary/' + this.updatedGlossary.id,
        updatedGlossary,
        { headers }
      )
      .subscribe({
        next: (response) => {
          // Update the glossary item in the list with the response
          const index = this.glossaries.findIndex(
            (item) => item.id === this.updatedGlossary.id
          );
          if (index !== -1) {
            this.glossaries[index] = response;
          }
        },
        error: (error) => {
          console.log(error);
        },
        complete: () => {
          this.saveChanges(modal);
        },
      });

    this.editModal?.hide(); // Hide the modal after updating the glossary item
  }

  // Save changes and hide the modal
  saveChanges(modal: any) {
    modal.hide();
  }

  // Delete a glossary entry
  deleteGlossary(glossary: any) {
    const token = localStorage.getItem('jwtToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http
      .delete<any>('https://localhost:5001/api/glossary/' + glossary.id, {
        headers,
      })
      .subscribe({
        next: (response) => {
          // Remove the deleted glossary item from the list
          this.glossaries = this.glossaries.filter(
            (item) => item.id !== glossary.id
          );
        },
        error: (error) => {
          console.log(error);
        },
      });
  }
}
