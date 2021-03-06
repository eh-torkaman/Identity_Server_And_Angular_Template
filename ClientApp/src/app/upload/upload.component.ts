import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { HttpEventType, HttpClient } from '@angular/common/http';
import { MessageService } from '../messageService/message.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})

export class UploadComponent implements OnInit {

  public progress: number;
  public message: string;

  @Output() public onUploadFinished = new EventEmitter();
  @Input() uploadUrl: string | undefined;
  constructor(private http: HttpClient, private messageService: MessageService) { }

  ngOnInit() {
  }

  public uploadFile = (files: any) => {
    let uploadUrl = this.uploadUrl as string;
    if (files.length === 0) {
      return;
    }
    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    this.http.post(uploadUrl, formData, { reportProgress: true, observe: 'events' })
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          let event_total=event.total as number
          this.progress = Math.round(100 * event.loaded / event_total );
        }
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          this.onUploadFinished.emit(event.body);
        }
      },
        err => this.messageService.NotifyErr(err));
  }
}
