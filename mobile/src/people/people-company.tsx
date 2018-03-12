import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { View, Text, ScrollView, Dimensions, Animated } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { DepartmentsHScrollableList } from './departments/departments-horizontal-scrollable-list';

interface PeopleCompanyProps {
    routeName: string;
}

const mapStateToProps = (state: AppState): PeopleCompanyProps => ({
    routeName: 'Company'
});

export class PeopleCompanyImpl extends React.Component<PeopleCompanyProps> {
    public render() {
        return <ScrollView>
            <DepartmentsHScrollableList />
            <DepartmentsHScrollableList />
            <DepartmentsHScrollableList />
        </ScrollView>;
    }
}

export const PeopleCompany = connect(mapStateToProps)(PeopleCompanyImpl);
