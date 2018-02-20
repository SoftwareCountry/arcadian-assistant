import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { PeopleActions, navigatePeopleRoom } from '../reducers/people/people.action';
import { loadEmployeesForRoom } from '../reducers/organization/organization.action';
import { EmployeesStore, EmployeeMap, EmployeeIdsGroupMap } from '../reducers/organization/employees.reducer';

interface PeopleRoomProps {
    employeesMap: EmployeeMap;
    roomNumber: string;
    employeesSubsetFilterCallback: any;
}

interface PeopleRoomDispatchProps {
    navigatePeopleRoom: (roomNumber: string) => void;
}

const mapStateToProps = (state: AppState): PeopleRoomProps => ({
    employeesMap: state.organization.employees.employeesById,
    roomNumber: state.userInfo.employee.roomNumber,
    employeesSubsetFilterCallback: state.people.employeesRoomSubsetFilterCallback
});

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    navigatePeopleRoom: (roomNumber: string) => { 
        dispatch(navigatePeopleRoom(roomNumber)); 
        dispatch(loadEmployeesForRoom(roomNumber));
    },
});

export class PeopleRoomImpl extends React.Component<PeopleRoomProps & PeopleRoomDispatchProps> {  
    public componentDidMount() {
        this.props.navigatePeopleRoom(this.props.roomNumber);
    }

    public render() {
        return <EmployeesList employees={this.props.employeesMap.toArray().filter(this.props.employeesSubsetFilterCallback)} />;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);