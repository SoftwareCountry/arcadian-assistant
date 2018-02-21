import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { View } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';

interface PeopleCompanyProps {
    routeName: string;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company'
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps> {  
    public render() {
        return <View style={{flex: 1, backgroundColor: '#fff'}}/>;
    }
}

export const PeopleCompany = connect(mapStateToProps)(PeopleCompanyImpl);