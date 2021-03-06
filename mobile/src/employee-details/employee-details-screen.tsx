import React, { Component } from 'react';
import { Department } from '../reducers/organization/department.model';
import { AppState } from '../reducers/app.reducer';
import { connect } from 'react-redux';
import { LayoutChangeEvent, RefreshControl, SafeAreaView, View, ViewStyle } from 'react-native';
import { EmployeeDetails } from './employee-details';
import { employeeDetailsStyles } from './styles';
import { NavigationScreenProps, ScrollView } from 'react-navigation';
import { LoadingView } from '../navigation/loading';
import Style from '../layout/style';
import { layoutStyles } from '../profile/styles';
import { Action, Dispatch } from 'redux';
import { refresh } from '../reducers/refresh/refresh.action';
import { Map, Set } from 'immutable';
import { EmployeeId } from '../reducers/organization/employee.model';
import { EmployeeMap } from '../reducers/organization/employees.reducer';
import { loadEmployees } from '../reducers/organization/organization.action';
import { equals } from '../utils/equitable';
import { loadCalendarEvents } from '../reducers/calendar/calendar.action';

//============================================================================
interface EmployeeDetailsProps {
    departments: Set<Department>;
    employees: EmployeeMap;
}

//============================================================================
interface EmployeeDetailsDispatchProps {
    refresh: () => void;
    loadEmployee: (employeeId: EmployeeId) => void;
    loadCalendarEvents: (employeeId: EmployeeId) => void;
}

//============================================================================
interface EmployeeDetailsState {
    width: number;
    height: number;
}

//============================================================================
class EmployeeDetailsScreenImpl extends Component<EmployeeDetailsProps & EmployeeDetailsDispatchProps & NavigationScreenProps, EmployeeDetailsState> {

    //----------------------------------------------------------------------------
    public constructor(props: Readonly<EmployeeDetailsProps & EmployeeDetailsDispatchProps & NavigationScreenProps>) {
        super(props);

        this.state = {
            width: 0,
            height: 0,
        };
    }

    //----------------------------------------------------------------------------
    public componentDidMount() {
        const employeeId: EmployeeId | undefined = this.props.navigation.getParam('employeeId', undefined);
        if (employeeId) {
            this.props.loadEmployee(employeeId);
        }
    }

    //----------------------------------------------------------------------------
    public shouldComponentUpdate(nextProps: EmployeeDetailsProps & EmployeeDetailsDispatchProps & NavigationScreenProps, nextState: EmployeeDetailsState) {
        if (this.state.width !== nextState.width || this.state.height !== nextState.height) {
            return true;
        }

        if (!equals(this.props.departments, nextProps.departments)) {
            return true;
        }

        const employeeId: EmployeeId | undefined = this.props.navigation.getParam('employeeId', undefined);
        const nextEmployeeId: EmployeeId | undefined = nextProps.navigation.getParam('employeeId', undefined);
        // noinspection RedundantIfStatementJS
        if (employeeId !== nextEmployeeId) {
            return true;
        }

        return false;
    }

    //----------------------------------------------------------------------------
    public render() {
        return <SafeAreaView style={Style.view.safeArea} onLayout={this.onLayout}>
            <View style={employeeDetailsStyles.container}>
                <View style={[
                    employeeDetailsStyles.topHalfView,
                    {
                        width: this.state.width,
                        height: this.state.height / 2,
                    }]}/>
                <View style={[
                    employeeDetailsStyles.bottomHalfView,
                    {
                        top: this.state.height / 2,
                        width: this.state.width,
                        height: this.state.height / 2
                    }]}/>
                {this.renderEmployeeDetails()}
            </View>
        </SafeAreaView>;
    }

    //----------------------------------------------------------------------------
    private renderEmployeeDetails(): JSX.Element {
        const employeeId: EmployeeId | undefined = this.props.navigation.getParam('employeeId', undefined);
        if (!employeeId) {
            return <LoadingView/>;
        }

        const employee = this.props.employees.get(employeeId);
        if (!employee) {
            return <LoadingView/>;
        }

        const department = this.props.departments.find(department => department.departmentId === employee.departmentId);
        if (!department) {
            return <LoadingView/>;
        }

        return (
            <ScrollView refreshControl={<RefreshControl tintColor={Style.color.white}
                                                        refreshing={false}
                                                        onRefresh={this.onRefresh}/>}>
                <EmployeeDetails
                    department={department}
                    employee={employee}
                    layoutStylesChevronPlaceholder={layoutStyles.chevronPlaceholder as ViewStyle}
                />
            </ScrollView>
        );
    }

    //----------------------------------------------------------------------------
    private onLayout = (event: LayoutChangeEvent) => {
        this.setState({
            width: event.nativeEvent.layout.width,
            height: event.nativeEvent.layout.height,
        });
    };

    //----------------------------------------------------------------------------
    private onRefresh = () => {
        const employeeId: EmployeeId | undefined = this.props.navigation.getParam('employeeId', undefined);
        if (employeeId) {
            this.props.loadCalendarEvents(employeeId);
        }
    };
}

//----------------------------------------------------------------------------
const stateToProps = (state: AppState): EmployeeDetailsProps => ({
    departments: state.organization ? Set(state.organization.departments) : Set(),
    employees: state.organization ? state.organization.employees.employeesById : Map(),
});

//----------------------------------------------------------------------------
const dispatchToProps = (dispatch: Dispatch<Action>): EmployeeDetailsDispatchProps => ({
    refresh: () => dispatch(refresh()),
    loadEmployee: employeeId => dispatch(loadEmployees([employeeId])),
    loadCalendarEvents: employeeId => dispatch(loadCalendarEvents(employeeId)),
});

export const EmployeeDetailsScreen = connect(stateToProps, dispatchToProps)(EmployeeDetailsScreenImpl);
