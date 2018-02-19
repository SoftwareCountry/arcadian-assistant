import React from 'react';
import { Action } from 'redux';
import { connect, Dispatch } from 'react-redux';
import { FlatList, Text, View, StyleSheet, ListRenderItemInfo } from 'react-native';

import { EmployeesList } from './employees-list';
import { AppState } from '../reducers/app.reducer';
import { PeopleActions, navigatePeopleRoom } from '../reducers/people/people.action';

interface PeopleRoomProps {
    roomNumber: string;
    employeesSubsetFilter: Function;
}

interface PeopleRoomDispatchProps {
    navigatePeopleRoom: (roomNumber: string) => void;
}

const mapStateToProps = (state: AppState): PeopleRoomProps => ({
    roomNumber: state.userInfo.employee.roomNumber,
    employeesSubsetFilter: state.people.employeesSubsetFilter
});

const mapDispatchToProps = (dispatch: Dispatch<PeopleActions>) => ({
    navigatePeopleRoom: (roomNumber: string) => { 
        dispatch(navigatePeopleRoom()); 
        console.log('PULL EMPLOYEES FOR ROOM WITH NUMBER ' + roomNumber );
        //dispatch(loadEmployeesForDepartment(departmentId));
    },
});

export class PeopleRoomImpl extends React.Component<PeopleRoomProps & PeopleRoomDispatchProps> {  
    public componentDidMount() {
        this.props.navigatePeopleRoom(this.props.roomNumber);
    }

    public render() {
        this.props.employeesSubsetFilter();
        return <EmployeesList />;
    }
}

export const PeopleRoom = connect(mapStateToProps, mapDispatchToProps)(PeopleRoomImpl);