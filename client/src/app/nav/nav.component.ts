import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
model: any = {}

  constructor(public accountService: AccountService, private router: Router,
    private toastr: ToastrService,private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
  }

  login(){
    this.spinner.show();
    this.accountService.login(this.model).subscribe({
      next: _ => {
        this.router.navigateByUrl('/glossaries'),
        this.spinner.hide();
    },
    error: (error) => {
      this.spinner.hide(); // Hide the spinner if login fails
      this.toastr.error(error);
      console.log(error);
    }
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
