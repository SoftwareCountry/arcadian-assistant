import React, { Component } from 'react';
import { Map, is } from 'immutable';
import { Avatar } from './avatar';
import { MapPhoto } from '../reducers/people/people.model';

interface CompanyDepartmentsLevelNodePhotoProps {
    photo: MapPhoto;
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