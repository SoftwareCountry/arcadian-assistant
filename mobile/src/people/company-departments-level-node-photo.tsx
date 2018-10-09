import React, { Component } from 'react';
import { Photo } from '../reducers/organization/employee.model';
import { Map, is } from 'immutable';
import { Avatar } from './avatar';

interface CompanyDepartmentsLevelNodePhotoProps {
    photo: Map<string, Photo>;
}

export class CompanyDepartmentsLevelNodePhoto extends Component<CompanyDepartmentsLevelNodePhotoProps> {
    public shouldComponentUpdate(nextProps: CompanyDepartmentsLevelNodePhotoProps) {
        return !is(this.props.photo, nextProps.photo);
    }

    public render() {
        const photo = this.props.photo 
            ? this.props.photo.toJS() 
            : null;

        return <Avatar photo={photo} />;
    }
}