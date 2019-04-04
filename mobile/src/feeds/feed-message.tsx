import React from 'react';
import { TouchableOpacity, View } from 'react-native';
import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/feeds/feed.model';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { FeedStyle } from './home-feeds-screen.styles';

//============================================================================
interface FeedMessageProps {
    message: Feed;
    employee?: Employee;
    onAvatarClicked?: (e: Employee) => void;
}

//============================================================================
export class FeedMessage extends React.Component<FeedMessageProps> {

    //----------------------------------------------------------------------------
    public render() {
        const message = this.props.message;
        const employee = this.props.employee;
        const photo = employee ? employee.photoUrl : undefined;
        const formattedDate = message.datePosted.format('MMMM D, YYYY');

        return (
            <View>
                <TouchableOpacity onPress={this.onAvatarClicked} style={FeedStyle.layout}>
                    <View style={FeedStyle.imgContainer}>
                        <Avatar photoUrl={photo}/>
                    </View>
                    <View style={FeedStyle.info}>
                        <StyledText style={FeedStyle.title}>{this.props.message.title}</StyledText>
                        <StyledText style={FeedStyle.text}>{this.props.message.text}</StyledText>
                        <StyledText style={FeedStyle.date}>{formattedDate}</StyledText>
                    </View>
                </TouchableOpacity>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private onAvatarClicked = () => {
        if (this.props.onAvatarClicked && this.props.employee) {
            this.props.onAvatarClicked(this.props.employee);
        }
    };
}
